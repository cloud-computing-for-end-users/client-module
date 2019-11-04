import * as React from "react";
import {WindowControls} from "../Shared/WindowControls";
import {BackendMethods} from "../../renderer";
const { ipcRenderer } = require('electron');
    
const spinner = require('../../../../assets/svg/spinner.svg');

interface IState {
  img: any;
  slaveKey: string;
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
    this.state = { 
      img: null,
      slaveKey: null
    };
  }

  componentDidMount(): void {
    ipcRenderer.send('call-backend-method', {
      method: BackendMethods.GetImagesFromSlave, 
      argument: {
        PrimaryKey: "123",// this.props.loggedInAs, 
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
      console.log("TEST; set-slaveKey-for-slaveAppWindow - remove me")
      ipcRenderer.send('set-slaveKey-for-slaveAppWindow', {slaveAppWindowKey: this.props.slaveAppWindowKey, slaveKey: this.state.slaveKey})
      
      setInterval(() => this.updateImage(json["PathToImages"]), 50); // time in ms
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetImagesFromSlave);
  }

  updateImage(imgPath: string): void {
    let path = imgPath + "?" + Date.now();
    let newImg = new Image();
    newImg.onerror=(() => console.log("Error on image occured"));    
    newImg.src=path;
    
    this.setState({
      img: path
    })
  }

  handleOnMouseDown(e: any): void {
    ipcRenderer.send('call-backend-method', {
      method: BackendMethods.MouseDown, 
      argument: {
        Button: this.GetButton(e.button),
        XinPercent: e.clientX / window.innerWidth * 100, 
        YinPercent: e.clientY / window.innerHeight * 100, 
        Key: this.state.slaveKey
      }
    });
  }
  
  handleOnMouseUp(e: any): void {
    ipcRenderer.send('call-backend-method', {
      method: BackendMethods.MouseUp, 
      argument: {
        Button: this.GetButton(e.button),
        XinPercent: e.clientX / window.innerWidth * 100, 
        YinPercent: e.clientY / window.innerHeight * 100, 
        Key: this.state.slaveKey
      }
    });
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
    if(this.state.img === null) {
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
            className="container-fluid m-0 p-0">
          <img draggable={false} src={this.state.img} />
        </div>
      )
    }
    
    return ([
      <WindowControls slaveAppKey={this.props.slaveAppWindowKey} showDragControl={true} key="WindowControls" />,
      <div key="SlaveView">{toRender}</div>
    ]);
  }
}
