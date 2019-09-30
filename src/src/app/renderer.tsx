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
    MouseMove = 'MouseMove',
    MouseScroll = 'MouseScroll'
}

if(document.getElementById('main')) {
    ReactDOM.render(<Main />, document.getElementById('main'));
}

if(document.getElementById('slave')) {
    ReactDOM.render(<Slave />, document.getElementById('slave'));
}