import * as React from "react";
import {ContentType} from "../Main";
import {NavigationItem} from "./NavigationItem";

const logo = require('../../../../../assets/img/ccfeu.png');
const settings = require('../../../../../assets/img/settings.png');

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
      <NavigationItem key={value.toString()} active={value == this.props.active} name={value} onViewChange={this.props.onViewChange} />
    );
    
    return (
      <div className="container-fluid px-0">
        <nav className="navbar navbar-expand navbar-light bg-light">
          <a className="navbar-brand" href="#">
            <img src={logo} width="40" height="40" className="d-inline-block align-top" alt="" />
          </a>
          <div className="navbar-collapse" id="navbarText">
            <ul className="navbar-nav mr-auto">
              {items}
            </ul>
            <span className="navbar-text">
              <img src={settings} width="30" height="30" className="d-inline-block align-top" alt="" />
            </span>
          </div>
        </nav>
      </div>
    );
  }
}
