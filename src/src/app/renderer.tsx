import * as ReactDOM from 'react-dom';
import * as React from 'react';
import {Dashboard} from "./components/Dashboard";
import {Button} from "./components/Button";

ReactDOM.render(<Dashboard />, document.getElementById('renderer'));
ReactDOM.render(<Button />, document.getElementById('button'));