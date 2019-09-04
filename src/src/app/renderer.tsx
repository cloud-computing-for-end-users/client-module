import * as ReactDOM from 'react-dom';
import * as React from 'react';
import * as $ from "jquery";
import 'bootstrap';
import {App} from "./components/App";
import '../scss/main.scss';

ReactDOM.render(<App />, document.getElementById('app'));

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})