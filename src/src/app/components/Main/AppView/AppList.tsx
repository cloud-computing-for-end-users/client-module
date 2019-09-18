import * as React from "react";
import {AppListItem} from "./AppListItem";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');
    
interface IState { 
  appListItems: any
}

interface IProps { }

export class AppList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);

    console.log(this.state);
    if(typeof this.state == 'undefined') {
      console.log("this.state is undefined")
    }

    this.state = {appListItems: <li>Initializing</li>};

    ipcRenderer.send('call-backend-method', {method: BackendMethods.GetListOfApplications, argument: ""});

    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetListOfApplications, (event, arg) => {
      var json = JSON.parse(arg);
      var items = [];
      for (var i = 0; i < json.length; i++){
        var obj = json[i];
        items.push(<AppListItem key={obj["ApplicationName"]} appName={obj["ApplicationName"]} appVersion={obj["ApplicationVersion"]} appOS={obj["RunningOnOperatingSystem"]} />)
      }
      this.setState({
        appListItems: items
      });
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetListOfApplications);
  }

  public render(): React.ReactNode {    
    return (
      <ul id="app-list" className="list-group">
        {this.state.appListItems}
      </ul>
    );
  }
}
