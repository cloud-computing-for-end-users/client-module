const url = require("url");
const path = require("path");

import { app, BrowserWindow, ipcMain } from "electron";

export class WindowManager {
    slaveWindows: BrowserWindow[] = [];
    window: BrowserWindow | null;

    createWindow = () => {
        // todo resizable set to true only for debugging
        this.window = new BrowserWindow({
            width: 500,
            height: 750,
            frame: false,
            resizable: true,
            webPreferences: {
                nodeIntegration: true
            }
        });

        this.window.loadURL(
            url.format({
                pathname: path.join(__dirname, "index.html"),
                protocol: "file:",
                slashes: true
            })
        );

        this.window.on("closed", () => {
            this.window = null;
            for (var i = this.slaveWindows.length; i > 0; i--) {
                this.slaveWindows[0].close();
            }
        });
    };

    createSlaveWindow = (width: number, height: number) => {
        let newSlaveWindow: any = new BrowserWindow({
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
            var index = this.slaveWindows.indexOf(newSlaveWindow);
            if (index !== -1) this.slaveWindows.splice(index, 1);
            newSlaveWindow = null;
        });

        newSlaveWindow.testProp = {'whatever': "I want"};

        this.slaveWindows.push(newSlaveWindow);
    }

    resizeWindow = (arg: any) => {
        BrowserWindow.fromId(arg.windowID).setSize(arg.width, arg.height);
    }
}