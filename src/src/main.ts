const url = require("url");
const path = require("path");

import { app, BrowserWindow, ipcMain } from "electron";

let window: BrowserWindow | null;

const createWindow = () => {
  window = new BrowserWindow({ 
    width: 500, 
    height: 750,
    frame: false,
    resizable: false,
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
  });
};

ipcMain.on('create-slave-window', (event, arg) => {
  createSlaveWindow(arg.width, arg.height);
});

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
    newSlaveWindow = null;
  });
}

app.on("ready", createWindow);

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