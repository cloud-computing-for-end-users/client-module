import { app, BrowserWindow } from "electron";
import { WindowManager } from "./windowManager";

export class ElectronManager {
    constructor(WindowHandler: WindowManager) {
        app.on("ready", () => {
            /*const path = require("path");
            const os = require('os');
            BrowserWindow.addDevToolsExtension(
                path.join(os.homedir(), 'AppData/Local/Google/Chrome/User Data/Default/Extensions/fmkadmapgofadopljbjfkapdkoienihi/4.1.2_0')
            );*/
            WindowHandler.createWindow();
        });

        app.on("window-all-closed", () => {
            if (process.platform !== "darwin") {
                app.quit();
            }
        });

        app.on("activate", () => {
            if (window === null) {
                WindowHandler.createWindow();
            }
        });
    }
}