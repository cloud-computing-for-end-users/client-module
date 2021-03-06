import * as React from "react";
import {AppListItem} from "./AppListItem";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');
    
const spinner = require('../../../../../assets/svg/spinner.svg');

interface IState {
  appListItems: any
}

interface IProps {
  loggedInAs: number
}

var savedAppListItems : any = null;

export class AppList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);

    if(savedAppListItems === null) {
      this.state = {appListItems: 
        <div className="container-fluid d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <span>Initializing</span>
        </div>
      };
      this.getAppListItems();
    } else {
      this.state = {appListItems: savedAppListItems};
    }
  }

  private getAppListItems() {
    ipcRenderer.send('call-backend-method', {method: BackendMethods.GetListOfApplications, argument: ""});
    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetListOfApplications, (event, arg) => {
      var json = JSON.parse(arg);
      var items = [];
      for (var i = 0; i < json.length; i++){
        var obj = json[i];
        items.push(<AppListItem key={obj["ApplicationName"]} loggedInAs={this.props.loggedInAs} appName={obj["ApplicationName"]} appVersion={obj["ApplicationVersion"]} appOS={obj["RunningOnOperatingSystem"]} />)
      }
      savedAppListItems = items;
      this.setState({
        appListItems: savedAppListItems
      });
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetListOfApplications);
  }

  render(): React.ReactNode {    
    return (
      <ul id="app-list" className="list-group list">
        {this.state.appListItems}
      </ul>
    );
  }
}
