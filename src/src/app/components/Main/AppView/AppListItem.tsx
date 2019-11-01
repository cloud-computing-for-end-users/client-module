import * as React from "react";
import * as $ from "jquery";
const appIcon = require('../../../../../assets/img/app.png');
const windowsIcon = require('../../../../../assets/img/windows.png');

interface IState { }

interface IProps {
  appName: string,
  appVersion: string,
  appOS: string,
  loggedInAs: number
}

export class AppListItem extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.newWindow = this.newWindow.bind(this);
  }

  newWindow() {
    const { ipcRenderer } = require('electron');
    // todo width and height
    ipcRenderer.send('create-slave-window', {
      width: 800, 
      height: 600,
      appName: this.props.appName, 
      appVersion: this.props.appVersion, 
      appOs: this.props.appOS, 
      loggedInAs: this.props.loggedInAs,
      slaveAppWindowKey: this.guidGenerator()
    });
  }

  // todo move to some util class
  private guidGenerator() {
    var S4 = function() {
       return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
    };
    return (S4()+"-"+S4()+"-"+S4()+"-"+S4());
  }

  componentDidMount(): void {
    $(function () {
      ($('[data-toggle="tooltip"]') as any).tooltip()
    });
  }
 
  public render(): React.ReactNode {
    var tooltipHtml = "Runs on " + this.props.appOS + " <img src=\"" + windowsIcon + '\" width="16" height="16" className="d-inline-block align-top" />';

    return (
      <li className="list-group-item d-flex align-items-center" data-toggle="tooltip" data-placement="bottom" data-html="true" data-delay='{"show":"1000", "hide":"0"}' 
          title={tooltipHtml}
          onClick={this.newWindow}>
        <img src={appIcon} className="d-inline-block align-top list-icon" />
        <div className="ml-2 d-flex flex-column">
          <span>{this.props.appName}</span>
          <span className="app-version">{this.props.appVersion}</span>
        </div>
      </li>
    );
  }
}
