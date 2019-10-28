import { BrowserWindow } from "electron";

const { ConnectionBuilder } = require("electron-cgi");

export class CGIConnectionManager {
    private connection: any;
    private mainWindow: BrowserWindow | null;
    private established: number;
    private mainWindowLoaded: boolean;
    
    constructor() {
        this.mainWindow = null;
        this.mainWindowLoaded = false;
        
        console.log('Creating new backend connection');
        this.newConnection();

        this.connection.onDisconnect = () => {
            this.updateEstablished(0);
            console.log('Backend connection lost, restarting...');
            this.newConnection();    
        };
    }

    private newConnection() {
        this.connection = new ConnectionBuilder().connectTo("dotnet", "run", "-nowarn", "--project", "./communication-module/communication-module").build();
        this.updateEstablished(1);
    }

    private updateEstablished = (established?: number) => {
        if(established !== undefined) {
            this.established = established;
        }
        if(this.mainWindowLoaded) {
            this.mainWindow.webContents.send('CGIConnection', this.established);
        }
    }

    callBackendMethod = (arg: any, event: any) => {
        var args = JSON.stringify(arg.argument);
        console.log("Calling backend method: " + arg.method + "(" + args + ")");
        this.connection.send(arg.method, args, (response: any) => {
            this.handleResponse(response);
            event.reply('reply-backend-method-' + arg.method, response);
        });
    }

    private handleResponse = (response: any) => {
        console.log("Response received: " + response);
        if (/\(PollingException\)/.test(response)) {
            const { dialog } = require('electron');
            dialog.showErrorBox("Polling Exception", response);
        } else if (/\(Exception\)/.test(response)) {
            const { dialog } = require('electron');
            dialog.showErrorBox("Exception", response);
        }
    }

    setMainWindow( mainWindow: BrowserWindow) {
        this.mainWindow = mainWindow;
        this.mainWindow.webContents.on('did-finish-load', () => {
            this.mainWindowLoaded = true;
            this.updateEstablished();
        })
    }
}