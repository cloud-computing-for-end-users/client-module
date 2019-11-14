import * as React from "react";
import {WindowControls} from "../Shared/WindowControls";
import {BackendMethods} from "../../renderer";
import { Utils } from "../../../utils/Utils";
const { ipcRenderer } = require('electron');
    
const spinner = require('../../../../assets/svg/spinner.svg');

interface IState {
  img: any,
  slaveKey: string,
  closing: boolean
}

interface IProps {
  appName: string,
  appVersion: string,
  appOS: string,
  loggedInAs: number,
  slaveAppWindowKey: number
}

export class Slave extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnMouseDown = this.handleOnMouseDown.bind(this);
    this.handleOnMouseUp = this.handleOnMouseUp.bind(this);
    this.onCloseSlaveAppWindow = this.onCloseSlaveAppWindow.bind(this);
    this.state = { 
      img: null,
      slaveKey: null,
      closing: false
    };
  }

  componentDidMount(): void {
    ipcRenderer.send('call-backend-method', {
      method: BackendMethods.GetImagesFromSlave, 
      argument: {
        PrimaryKey: Utils.getLoggedInAs(this.props.loggedInAs),
        ApplicationName: this.props.appName, 
        ApplicationVersion: this.props.appVersion, 
        RunningOnOperatingSystem: this.props.appOS
      }
    });
    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetImagesFromSlave, (event, arg) => {
      var json = JSON.parse(arg);
      ipcRenderer.send('resize-slave-window', {width: json["WindowWidth"], height: json["WindowHeight"], windowID: require('electron').remote.getCurrentWindow().id});
      this.setState({
        slaveKey: json["SlaveKey"]
      });
      
      ipcRenderer.send('setSlaveKeyForSlaveAppWindow', {slaveAppWindowKey: this.props.slaveAppWindowKey, slaveKey: this.state.slaveKey})
      
      document.addEventListener("keydown", this.handleOnKeyDown.bind(this));
      document.addEventListener("keyup", this.handleOnKeyUp.bind(this));

      setInterval(() => this.updateImage(json["PathToImages"]), 50); // time in ms
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetImagesFromSlave);
  }

  private updateImage(imgPath: string): void {
    let path = imgPath + "?" + Date.now();
    let newImg = new Image();
    newImg.onerror=(() => console.log("Error on image occured"));    
    newImg.src=path;
    
    this.setState({
      img: path
    })
  }

  private handleOnMouseDown(e: any): void {
    this.handleOnMouse(true, e);
  }
  
  private handleOnMouseUp(e: any): void {
    this.handleOnMouse(false, e);
  }

  private handleOnMouse(isMouseDown: boolean, e: any) {
    ipcRenderer.send('call-backend-method', {
      method: isMouseDown ? BackendMethods.MouseDown : BackendMethods.MouseUp, 
      argument: {
        Button: this.GetButton(e.button),
        XinPercent: e.clientX / window.innerWidth * 100, 
        YinPercent: e.clientY / window.innerHeight * 100, 
        SlaveKey: this.state.slaveKey
      }
    });
  }

  private handleOnKeyDown(e: any): void {
    console.log(e)
    this.handleOnKey(true, e.key)
  }

  private handleOnKeyUp(e: any): void {
    this.handleOnKey(false, e.key)
  }

  private handleOnKey(isKeyDown: boolean, keyCode: string): void {
    ipcRenderer.send('call-backend-method', {
      method: isKeyDown ? BackendMethods.KeyDown : BackendMethods.KeyUp, 
      argument: {
        Key: keyCode, 
        SlaveKey: this.state.slaveKey
      }
    });
  }

  private onCloseSlaveAppWindow(closing: boolean) {
    this.setState({closing});
  }

  private GetButton(button: any): string {
    if(button == 0) {
      return "Left"; 
    } else if(button == 2) {
      return "Right"; 
    } else {
      return "Undefined";
    }
  }

  render(): React.ReactNode {
    var toRender;
    if(this.state.closing) {
      toRender = (
        <div className="container-fluid vh-100 d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <h1>Saving files and closing</h1>
        </div>
      );
    } else if(this.state.img === null) {
      toRender = (
        <div className="container-fluid vh-100 d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <h1>Initializing</h1>
        </div>
      ); 
    } else {
      toRender = (
        <div
            onMouseUp={this.handleOnMouseUp} 
            onMouseDown={this.handleOnMouseDown} 
            onKeyDown={this.handleOnKeyDown}
            className="container-fluid m-0 p-0">
          <img draggable={false} src={this.state.img} />
        </div>
      )
    }
    
    return ([
      <WindowControls onCloseSlaveAppWindow={[this.onCloseSlaveAppWindow, this.state.slaveKey]} showDragControl={true} key="WindowControls" />,
      <div key="SlaveView">{toRender}</div>
    ]);
  }
}
