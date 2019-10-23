import * as React from "react";
import * as $ from "jquery";
const app = require('../../../../../assets/img/app.png');
const windows = require('../../../../../assets/img/windows.png');

interface IState { }

interface IProps {
  appName: string,
  appVersion: string,
  appOS: string,
  primaryKey: number
}

export class AppListItem extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.newWindow = this.newWindow.bind(this);
  }

  public newWindow() {
    const { ipcRenderer } = require('electron');
    // todo width and height
    ipcRenderer.send('create-slave-window', {
      width: 800, 
      height: 600,
      appName: this.props.appName, 
      appVersion: this.props.appVersion, 
      appOs: this.props.appOS, 
      primaryKey: this.props.primaryKey
    });
  }

  public componentDidMount(): void {
    $(function () {
      ($('[data-toggle="tooltip"]') as any).tooltip()
    });
  }
 
  public render(): React.ReactNode {
    var tooltipHtml = "Runs on " + this.props.appOS + " <img src=\"" + windows + '\" width="16" height="16" className="d-inline-block align-top" alt="" />';

    return (
      <li className="list-group-item d-flex align-items-center" data-toggle="tooltip" data-placement="bottom" data-html="true" data-delay='{"show":"1000", "hide":"0"}' 
          title={tooltipHtml}
          onClick={this.newWindow}>
        <img src={app} width="42" height="42" className="d-inline-block align-top" alt="" />
        <div className="ml-2 d-flex flex-column">
          <span>{this.props.appName}</span>
          <span className="app-version">{this.props.appVersion}</span>
        </div>
      </li>
    );
  }
}
