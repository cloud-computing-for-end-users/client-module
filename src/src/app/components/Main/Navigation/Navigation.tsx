import * as React from "react";
import {ContentType} from "../Main";
import {NavigationItem} from "./NavigationItem";
import {NavigationSettings} from "./NavigationSettings";

const logo = require('../../../../../assets/img/ccfeu.png');

interface IState { }

interface IProps { 
  active: ContentType,
  onViewChange: any
}

export class Navigation extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    var values = Object.values(ContentType)
    const items = values.map((value) =>
      <NavigationItem key={value} active={value == this.props.active} name={value} onViewChange={this.props.onViewChange} />
    );
    
    return (
      <div className="container-fluid px-0">
        <nav className="navbar navbar-expand">
          <div className="navbar-brand">
            <img src={logo} width="40" height="40" className="d-inline-block align-top" alt="" />
          </div>
          <div className="navbar-collapse" id="navbarText">
            <ul className="navbar-nav mr-auto">
              {items}
            </ul>
            <NavigationSettings />
          </div>
        </nav>
      </div>
    );
  }
}
