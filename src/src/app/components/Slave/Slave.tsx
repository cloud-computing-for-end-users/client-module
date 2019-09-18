import * as React from "react";
import {WindowControls} from "../Shared/WindowControls";
import {BackendMethods} from "../../renderer";

const spinner = require('../../../../assets/svg/spinner.svg');

interface IState {
  imgPath: string;
  imgHash: number;
}

interface IProps { }

export class Slave extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnMouseDown = this.handleOnMouseDown.bind(this);
    this.handleOnMouseUp = this.handleOnMouseUp.bind(this);
    this.handleOnWheel = this.handleOnWheel.bind(this);
    this.state = { 
      imgPath: "",
      imgHash: Date.now()
    };
  }

  componentDidMount(): void {
    const { ipcRenderer } = require('electron');
    ipcRenderer.send('call-backend-method', {method: BackendMethods.GetPathToImage, argument: ""});

    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetPathToImage, (event, arg) => {
      this.setState({ imgPath: arg });
      setInterval(() => this.updateImage(), 50);
    })
  }

  public updateImage(): void {
    this.setState({
      imgHash: Date.now()
    })
  }

  handleOnMouseDown(e: any): void {
    console.log("Down ", e.clientX, e.clientY);
  }
  
  handleOnMouseUp(e: any): void {
    console.log("Up ", e.clientX, e.clientY);
  }

  handleOnWheel(e: any): void {
    console.log(e.deltaX, e.deltaY);
  }

  public render(): React.ReactNode {
    var toRender;
    if(this.state.imgPath == "") {
      toRender = (
        <div className="container-fluid vh-100 d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <h1>Initializing</h1>
        </div>
      ); 
    } else {
      toRender = (
        <div onWheel={this.handleOnWheel} onMouseUp={this.handleOnMouseUp} onMouseDown={this.handleOnMouseDown} className="container-fluid m-0 p-0">
          <img src={`${this.state.imgPath}?${this.state.imgHash}`} />
        </div>
      )
    }
    
    return ([
      <WindowControls key="WindowControls" />,
      <div key="SlaveView">{toRender}</div>
    ]);
  }
}
