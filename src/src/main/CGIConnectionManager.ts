const { ConnectionBuilder } = require("electron-cgi");

export class CGIConnectionManager {
    private connection: any;
    
    constructor() {
        console.log('Creating new backend connection');
        this.newConnection();

        this.connection.onDisconnect = () => {
            console.log('Backend connection lost, restarting...');
            this.newConnection();    
        };
    }

    private newConnection() {
        this.connection = new ConnectionBuilder().connectTo("dotnet", "run", "-nowarn", "--project", "./communication-module/communication-module").build();
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
}