import * as React from "react";
import {WindowControls} from "../Shared/WindowControls";
import {BackendMethods} from "../../renderer";
const { ipcRenderer } = require('electron');
    
const spinner = require('../../../../assets/svg/spinner.svg');

interface IState {
  img: any;
  key: string;
}

interface IProps { }

export class Slave extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnMouseDown = this.handleOnMouseDown.bind(this);
    this.handleOnMouseUp = this.handleOnMouseUp.bind(this);
    this.handleOnWheel = this.handleOnWheel.bind(this);
    this.state = { 
      img: null,
      key: null
    };
  }

  componentDidMount(): void {
    ipcRenderer.send('call-backend-method', {method: BackendMethods.GetImagesFromSlave, argument: ""});

    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetImagesFromSlave, (event, arg) => {
      var json = JSON.parse(arg);
      ipcRenderer.send('resize-slave-window', {width: json["WindowWidth"], height: json["WindowHeight"], windowID: require('electron').remote.getCurrentWindow().id});
      this.setState({
        key: json["SlaveKey"]
      });
      setInterval(() => this.updateImage(json["PathToImages"]), 2000);
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetImagesFromSlave);
  }

  public updateImage(imgPath: string): void {
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
        XinPercent: e.clientX / window.innerWidth * 100, 
        YinPercent: e.clientY / window.innerHeight * 100, 
        key: this.state.key
      }
    });
    // todo remove this logging, used only for debugging
    console.log("Down ", e.clientX / window.innerWidth, e.clientY / window.innerHeight, this.state.key);
  }
  
  handleOnMouseUp(e: any): void {
    console.log("Up ", e.clientX, e.clientY);
  }

  handleOnWheel(e: any): void {
    console.log(e.deltaX, e.deltaY);
  }

  public render(): React.ReactNode {
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
        <div onWheel={this.handleOnWheel} onMouseUp={this.handleOnMouseUp} onMouseDown={this.handleOnMouseDown} className="container-fluid m-0 p-0">
          <img draggable={false} src={this.state.img} />
        </div>
      )
    }
    
    return ([
      <WindowControls showDragControl={true} key="WindowControls" />,
      <div key="SlaveView">{toRender}</div>
    ]);
  }
}
