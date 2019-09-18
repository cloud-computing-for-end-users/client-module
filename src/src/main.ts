const url = require("url");
const path = require("path");
const os = require('os');

import { app, BrowserWindow, ipcMain } from "electron";
import { BackendMethods } from "./app/renderer";

let window: BrowserWindow | null;
let slaveWindows: BrowserWindow[] = [];

const createWindow = () => {
  // todo resizable set to true only for debugging
  window = new BrowserWindow({ 
    width: 500, 
    height: 750,
    frame: false,
    resizable: true,
    webPreferences: {
      nodeIntegration: true
    }
  });

  window.loadURL(
    url.format({
      pathname: path.join(__dirname, "index.html"),
      protocol: "file:",
      slashes: true
    })
  );

  window.on("closed", () => {
    window = null;
    for(var i = slaveWindows.length; i > 0; i--) {
      slaveWindows[0].close();
    }
  });
};

ipcMain.on('create-slave-window', (event, arg) => {
  createSlaveWindow(arg.width, arg.height);
});

ipcMain.on('call-backend-method', (event, arg) => {
  console.log("Calling backend method: " + arg.method + "(" + arg.argument + ")");
  callBackendMethod(event, arg.method, arg.argument);
});

const { ConnectionBuilder } = require("electron-cgi");
console.log('Creating new backend connection');
let connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();

connection.onDisconnect = () => {
  console.log('Backend connection lost, restarting...');
  connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();
};

const callBackendMethod = (event: Electron.IpcMainEvent, method: BackendMethods, argument: string) => {  
  connection.send(method, argument, (response: any) => {
    console.log("Response received: " + response);
    event.reply('reply-backend-method-' + method, response);
  });
}

const createSlaveWindow = (width: number, height: number) => {
  // todo resizable shouldn't be false, but it's easier for now
  let newSlaveWindow = new BrowserWindow({
    width: width,
    height: height,
    frame: false,
    resizable: true,
    webPreferences: {
      nodeIntegration: true
    }
  });

  newSlaveWindow.loadURL(
    url.format({
      pathname: path.join(__dirname, "slave.html"),
      protocol: "file:",
      slashes: true
    })
  );

  newSlaveWindow.on("closed", () => {
    console.log("Closing window");
    var index = slaveWindows.indexOf(newSlaveWindow);
    if (index !== -1) slaveWindows.splice(index, 1);
    newSlaveWindow = null;
  });

  slaveWindows.push(newSlaveWindow);
}

app.on("ready", () => {
  BrowserWindow.addDevToolsExtension(
    path.join(os.homedir(), 'AppData/Local/Google/Chrome/User Data/Default/Extensions/fmkadmapgofadopljbjfkapdkoienihi/4.0.6_0')
  );  
  createWindow();
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  if (window === null) {
    createWindow();
  }
});