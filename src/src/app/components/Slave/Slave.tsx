import * as React from "react";
import * as $ from "jquery";
import {WindowControls} from "../Shared/WindowControls";

const { ConnectionBuilder } = require("electron-cgi");

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

  disableDragging(): void {
    $('img').prop('draggable', false);
  }

  componentDidUpdate(): void {
    this.disableDragging();
  }

  componentDidMount(): void {
    this.disableDragging();
    
    let connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();

    connection.onDisconnect = () => {
      alert('Connection lost, restarting...');
      connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();
    };

    connection.send("getPathToImage", "from C#", (response: any) => {
      this.setState({ imgPath: response });
      setInterval(() => this.updateImage(), 50);
    });
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
      toRender = "Initializing";
    } else {
      toRender = <img src={`${this.state.imgPath}?${this.state.imgHash}`} />
    }
    
    return ([
      <WindowControls key="WindowControls" />,
      <div key="SlaveView" onWheel={this.handleOnWheel} onMouseUp={this.handleOnMouseUp} onMouseDown={this.handleOnMouseDown} className="container-fluid m-0 p-0">
        {toRender}
      </div>
    ]);
  }
}
