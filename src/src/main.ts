const url = require("url");
const path = require("path");

import { app, BrowserWindow, ipcMain } from "electron";

let window: BrowserWindow | null;
let slaveWindow: BrowserWindow[] | null;

const createWindow = () => {
  window = new BrowserWindow({ 
    width: 500, 
    height: 750,
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
  createSlaveWindow();
});

const createSlaveWindow = () => {
  // todo remove debugging console.log

  let newSlaveWindow = new BrowserWindow({
    width: 300,
    height: 300,
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

  /*
  slaveWindow.push(newSlaveWindow);

  console.log("2. Count: " + slaveWindow.length);

  slaveWindow[slaveWindow.length - 1].loadURL(
    url.format({
      pathname: path.join(__dirname, "slave.html"),
      protocol: "file:",
      slashes: true
    })
  );

  slaveWindow[slaveWindow.length - 1].on("closed", () => {
    console.log("3. Count: " + slaveWindow.length);
    slaveWindow.pop();
    console.log("4. Count: " + slaveWindow.length);
  });

  */
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