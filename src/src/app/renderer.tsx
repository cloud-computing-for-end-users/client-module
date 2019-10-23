import * as ReactDOM from 'react-dom';
import * as React from 'react';
import 'bootstrap';
import {Main} from "./components/Main/Main";
import {Slave} from "./components/Slave/Slave";
import '../scss/main.scss';

export enum BackendMethods {
    GetListOfApplications = "GetListOfApplications",
    GetImagesFromSlave = 'GetImagesFromSlave',
    MouseDown = 'MouseDown',
    MouseUp = 'MouseUp',
    Login = 'Login',
    CreateAccount = 'CreateAccount'
}

if(document.getElementById('main')) {
    ReactDOM.render(<Main />, document.getElementById('main'));
}

if(document.getElementById('slave')) {
    var electron = require('electron');
    var currentWindow: any = electron.remote.getCurrentWindow();
     
    console.log(currentWindow.custom);

    //ReactDOM.render(<Slave />, document.getElementById('slave'));
}