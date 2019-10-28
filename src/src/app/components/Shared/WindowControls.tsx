import * as React from "react";
import appsMap from "../Main/Main";

interface IState { }
interface IProps { 
  showDragControl: boolean,
  slaveAppKey: number | null 
}

export class WindowControls extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleCloseControl = this.handleCloseControl.bind(this); 
    this.handleMinimizeControl = this.handleMinimizeControl.bind(this); 
  }

  public handleCloseControl(e: any) {
    if(this.props.slaveAppKey !== null) {
      appsMap.delete(this.props.slaveAppKey);
      console.log(appsMap);
    }
    //const { remote } = require('electron')
    //remote.BrowserWindow.getFocusedWindow().close();
  }

  public handleMinimizeControl(e: any) {
    const { remote } = require('electron')
    remote.BrowserWindow.getFocusedWindow().minimize();
  }

  public render(): React.ReactNode {
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