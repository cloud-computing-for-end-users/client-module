const url = require("url");
const path = require("path");
import { CGIConnectionManager } from "./CGIConnectionManager";

import { BrowserWindow } from "electron";

export class WindowManager {
    slaveWindows: BrowserWindow[] = [];
    mainWindow: BrowserWindow | null;
    CGIConnectionHandler: CGIConnectionManager;

    constructor(CGIConnectionHandler: CGIConnectionManager) {
        this.CGIConnectionHandler = CGIConnectionHandler;
    }

    createWindow = () => {
        // todo resizable set to true only for debugging
        this.mainWindow = new BrowserWindow({
            width: 500,
            height: 750,
            frame: false,
            resizable: true,
            webPreferences: {
                nodeIntegration: true
            }
        });

        this.mainWindow.loadURL(
            url.format({
                pathname: path.join(__dirname, "index.html"),
                protocol: "file:",
                slashes: true
            })
        );

        this.mainWindow.on("closed", () => {
            this.mainWindow = null;
            for (var i = this.slaveWindows.length; i > 0; i--) {
                this.slaveWindows[0].close();
            }
        });

        this.CGIConnectionHandler.setMainWindow(this.mainWindow);
    };

    createSlaveWindow = (width: number, height: number, appName: string, appVersion: string, appOs: string, loggedInAs: number) => {
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

        newSlaveWindow.slaveWindowProps = {'appName': appName, 'appVersion': appVersion, 'appOs': appOs, 'loggedInAs': loggedInAs};

        this.slaveWindows.push(newSlaveWindow);
    }

    resizeWindow = (arg: any) => {
        BrowserWindow.fromId(arg.windowID).setSize(arg.width, arg.height);
    }
}