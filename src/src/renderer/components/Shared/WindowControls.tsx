import * as React from "react";
import { ipcRenderer } from "electron";
import { BackendMethods } from "../../renderer";

interface IState { }
interface IProps { 
  showDragControl: boolean,
  onCloseSlaveAppWindow: any | null 
}

export class WindowControls extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleCloseControl = this.handleCloseControl.bind(this); 
    this.handleMinimizeControl = this.handleMinimizeControl.bind(this); 
  }

  handleCloseControl(e: any) {
    const { remote } = require('electron')
    if(this.props.onCloseSlaveAppWindow !== null) {
      this.props.onCloseSlaveAppWindow[0](true);
      ipcRenderer.send('call-backend-method', {method: BackendMethods.SaveFilesAndTerminate, argument: {SlaveKey: this.props.onCloseSlaveAppWindow[1]}});
      ipcRenderer.on('reply-backend-method-' + BackendMethods.SaveFilesAndTerminate, (event, arg) => {
        remote.BrowserWindow.getFocusedWindow().close();
      })
    } else {
      remote.BrowserWindow.getFocusedWindow().close();
    }
  }

  handleMinimizeControl(e: any) {
    const { remote } = require('electron')
    remote.BrowserWindow.getFocusedWindow().minimize();
  }

  render(): React.ReactNode {
    return ( 
        <div className="window-controls">
            {this.props.showDragControl && 
              <a className="drag-control"></a>
            }
            <a onClick={this.handleMinimizeControl} className="minimize-control"></a>
            <a onClick={this.handleCloseControl} className="close-control"></a>
        </div>
    );
  }
}