import * as ReactDOM from 'react-dom';
import * as React from 'react';
import 'bootstrap';
import {Main} from "./components/Main/Main";
import {Slave} from "./components/Slave/Slave";
import '../scss/main.scss';

export enum BackendMethods {
    // DevUtils
    EstablishCGIConnection = "EstablishCGIConnection",
    // EntryView
    Login = 'Login',
    CreateAccount = 'CreateAccount',
    // AppView
    GetListOfApplications = "GetListOfApplications",
    // FileView
    GetListOfFiles = 'GetListOfFiles',
    UploadFile = 'UploadFile',
    DownloadFile = 'DownloadFile',
    RenameFile = 'RenameFile',
    RemoveFile = 'RemoveFile',
    TellSlaveToFetchFile = 'TellSlaveToFetchFile',
    SaveFilesAndTerminate = 'SaveFilesAndTerminate',
    // Slave
    GetImagesFromSlave = 'GetImagesFromSlave',
    MouseMove = 'MouseMove',
    MouseDown = 'MouseDown',
    MouseUp = 'MouseUp',
    KeyDown = 'KeyDown',
    KeyUp = 'KeyUp'
}

if(document.getElementById('main')) {
    ReactDOM.render(<Main />, document.getElementById('main'));
}

if(document.getElementById('slave')) {
    var electron = require('electron');
    var currentWindow: any = electron.remote.getCurrentWindow();
    var p: any = currentWindow.slaveWindowProps;
    ReactDOM.render(<Slave appName={p.appName} appVersion={p.appVersion} appOS={p.appOs} loggedInAs={p.loggedInAs} slaveAppWindowKey={p.slaveAppWindowKey} />, document.getElementById('slave'));
}