import * as React from "react";
import { ipcRenderer } from "electron";
const { ConnectionBuilder } = require("electron-cgi");

interface IState {
  message: string;
}

export class Dashboard extends React.Component<{}, IState> {
  public state: IState = {
    message: ""
  };

  public componentDidMount(): void {
    let connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();

    connection.onDisconnect = () => {
      alert('Connection lost, restarting...');
      connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();
    };

    connection.send("greeting", "from C#", (response: any) => {
      this.setState({ message: response });
    });
  }

  public componentWillUnmount(): void {
    
  }

  public render(): React.ReactNode {
    return <h1>State: {this.state.message}</h1>;
  }
}
