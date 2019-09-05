import * as React from "react";
import * as $ from "jquery";
const app = require('../../../../../assets/img/app.png');
const windows = require('../../../../../assets/img/windows.png');

interface IState { }

interface IProps { }

export class AppListItem extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.newWindow = this.newWindow.bind(this);
  }

  public newWindow() {
    const { ipcRenderer } = require('electron');
    // todo width and height
    ipcRenderer.send('create-slave-window', 800, 600);
  }

  public componentDidMount(): void {
    $(function () {
      ($('[data-toggle="tooltip"]') as any).tooltip()
    })
  }

  public render(): React.ReactNode {
    var tooltipHtml = "Runs on <img src=\"" + windows + '\" width="16" height="16" className="d-inline-block align-top" alt="" />';

    return (
      <li className="list-group-item d-flex align-items-center" data-toggle="tooltip" data-placement="bottom" data-html="true" data-delay='{"show":"1000", "hide":"0"}' 
          title={tooltipHtml}
          onClick={this.newWindow}>
        <img src={app} width="30" height="30" className="d-inline-block align-top" alt="" />
        <span className="ml-2">App Name</span>
      </li>
    );
  }
}
