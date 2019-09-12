import * as ReactDOM from 'react-dom';
import * as React from 'react';
import 'bootstrap';
import {Main} from "./components/Main/Main";
import {Slave} from "./components/Slave/Slave";
import '../scss/main.scss';

// this is used in main.ts but exporting enum for some reason failed there but in here
export enum BackendMethods {
    GetPathToImage = "getPathToImage"
}

if(document.getElementById('main')) {
    ReactDOM.render(<Main />, document.getElementById('main'));
}

if(document.getElementById('slave')) {
    ReactDOM.render(<Slave />, document.getElementById('slave'));
}