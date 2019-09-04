import * as React from "react";
const logo =  require('../../../assets/img/ccfeu.png');
const settings =  require('../../../assets/img/settings.png');
const app =  require('../../../assets/img/app.png');
const windows =  require('../../../assets/img/windows.png');

const { ConnectionBuilder } = require("electron-cgi");

interface IState {
  message: string;
}

interface IProps { }

export class App extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = { message: "" };

    // This binding is necessary to make `this` work in the callback
    this.newWindow = this.newWindow.bind(this);
  }

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

  newWindow() {
    const { ipcRenderer } = require('electron');
    ipcRenderer.send('create-slave-window');
  }

  public render(): React.ReactNode {
    var tooltipHtml = "Runs on <img src=\"" + windows + '\" width="16" height="16" className="d-inline-block align-top" alt="" />';
    
    return (
      <div>  
        <div className="container-fluid px-0">
          <nav className="navbar navbar-expand navbar-light bg-light">
            <a className="navbar-brand" href="#">
              <img src={logo} width="40" height="40" className="d-inline-block align-top" alt="" />
            </a>
            <div className="navbar-collapse" id="navbarText">
              <ul className="navbar-nav mr-auto">
                <li className="nav-item active">
                  <a className="nav-link" href="#">Apps <span className="sr-only">(current)</span></a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="#">Files</a>
                </li>
              </ul>
              <span className="navbar-text">
                <img src={settings} width="30" height="30" className="d-inline-block align-top" alt="" />
              </span>
            </div>
          </nav>
        </div>
        <div className="container">
          <div className="row">
            <ul className="list-group mt-3">
              <li className="list-group-item d-flex align-items-center" data-toggle="tooltip" data-placement="bottom" data-html="true" data-delay='{"show":"1000", "hide":"0"}' 
                  title={tooltipHtml}>
                <img src={app} width="30" height="30" className="d-inline-block align-top" alt="" />
                <span className="ml-2">App Name</span>
              </li>
            </ul>
          </div>
          <button type="button" className="btn btn-secondary" onClick={this.newWindow}>
            Tooltip on left
          </button>
        </div>
      </div>
    );
  }
}
