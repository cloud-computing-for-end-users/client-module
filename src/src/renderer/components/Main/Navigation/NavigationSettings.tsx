import * as React from "react";
import FeatureFlags from "../../../../utils/FeatureFlags";

const settings = require('../../../../../assets/img/settings.png');

interface IState { }

interface IProps {
  onLoggedInChange: any
  email: string
}

export class NavigationSettings extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClick = this.handleOnClick.bind(this); 
  }

  handleOnClick(e: any) {
    this.props.onLoggedInChange(false, 0, "");
  }

  render(): React.ReactNode {
    var options = ["Logout"];
    const items = options.map((option) =>
      <a key={option} onClick={this.handleOnClick} className="dropdown-item" href="#">{option}</a>
    );
    
    return (
      <span className="navbar-text">
        <div className="dropdown">
          <button className="btn" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            <img src={settings} width="30" height="30" className="d-inline-block align-top" />
          </button>
          <div className="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenuButton">
            {FeatureFlags.AllowLogin &&
              <h6 className="dropdown-header">{this.props.email}</h6>
            }
            {items}
          </div>
        </div>
      </span>
    );
  }
}
