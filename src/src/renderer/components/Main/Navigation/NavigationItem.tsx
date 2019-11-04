import * as React from "react";
import {ContentType} from "../Main";

interface IState { }

interface IProps { 
  active: boolean,
  name: ContentType,
  onViewChange: any
}

export class NavigationItem extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleChange = this.handleChange.bind(this); 
  }

  handleChange() {
    this.props.onViewChange(this.props.name);
  }

  render(): React.ReactNode {   
    return (
      <li onClick={this.handleChange} className={"nav-item" + (this.props.active ? " active" : "")} >
        <a className="nav-link" href="#">{this.props.name}</a>
      </li>
    );
  }
}
