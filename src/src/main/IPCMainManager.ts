import { ipcMain } from "electron";
import { WindowManager } from "./windowManager";
import { CGIConnectionManager } from "./CGIConnectionManager";

export class IPCMainManager {
    constructor(WindowHandler: WindowManager, CGIConnectionHandler: CGIConnectionManager) {
        ipcMain.on('create-slave-window', (event, arg) => {
            WindowHandler.createSlaveWindow(arg.width, arg.height, arg.appName, arg.appVersion, arg.appOs, arg.primaryKey, arg.appKey);
        });

        ipcMain.on('resize-slave-window', (event, arg) => {
            WindowHandler.resizeWindow(arg);
        })

        ipcMain.on('call-backend-method', (event, arg) => {
            CGIConnectionHandler.callBackendMethod(arg, event);
        });
    }
}