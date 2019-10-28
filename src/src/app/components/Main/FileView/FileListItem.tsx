import * as React from "react";
import * as $ from "jquery";
const fileIcon = require('../../../../../assets/img/file.png');

interface IState { }

interface IProps {
  fileName: string
}

export class FileListItem extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClick = this.handleOnClick.bind(this);
  }

  private handleOnClick(e: any) {
    if(!e.currentTarget.className.includes("active")) {
      $("#file-list li").removeClass("active");
    }
    $(e.currentTarget).toggleClass("active");
  }
 
  render(): React.ReactNode {
    return (
      <li onClick={this.handleOnClick} className="list-group-item d-flex align-items-center">
        <img src={fileIcon} className="d-inline-block align-top list-icon" />
        <div className="ml-2 d-flex flex-column">
          <span>{this.props.fileName}</span>
        </div>
      </li>
    );
  }
}
