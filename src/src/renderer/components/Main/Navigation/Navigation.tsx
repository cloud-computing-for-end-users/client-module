import * as React from "react";
import {ContentType} from "../Main";
import {NavigationItem} from "./NavigationItem";
import {NavigationSettings} from "./NavigationSettings";
import {CGIConnectionStatus, StatusTitle} from "../../Shared/CGIConnectionStatus";
import FeatureFlags from "../../../../utils/FeatureFlags";

const logo = require('../../../../../assets/img/ccfeu.png');

interface IState { }

interface IProps { 
  active: ContentType,
  onViewChange: any,
  onLoggedInChange: any,
  email: string
}

export class Navigation extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  render(): React.ReactNode {
    var values = Object.values(ContentType)
    const items = values.map((value) =>
      <NavigationItem key={value} active={value == this.props.active} name={value} onViewChange={this.props.onViewChange} />
    );
    
    return (
      <div className="container-fluid px-0">
        <nav className="navbar navbar-expand">
          <div className="navbar-brand">
            <img src={logo} width="40" height="40" className="d-inline-block align-top" />
          </div>
          <div className="navbar-collapse" id="navbarText">
            <ul className="navbar-nav mr-auto">
              {items}
            </ul>
            {FeatureFlags.ShowConnectionStatus && 
              <CGIConnectionStatus title={StatusTitle.ServerModule} />
            }
            <NavigationSettings email={this.props.email} onLoggedInChange={this.props.onLoggedInChange} />
          </div>
        </nav>
      </div>
    );
  }
}
