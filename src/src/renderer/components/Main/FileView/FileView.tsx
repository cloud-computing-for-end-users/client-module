import * as React from "react";
import {FileList} from "./FileList";
import {FileListButtons} from "./FileListButtons";
    
const spinner = require('../../../../../assets/svg/spinner.svg');

interface IState {}

interface IProps { 
  loggedInAs: number,
  fileListItems: any,
  getFileListItems: any
}

export class FileView extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  componentDidMount(): void {
    if(this.props.fileListItems === null) {
      this.props.getFileListItems();
    }
  }

  render(): React.ReactNode {
    var toRender;
    if(this.props.fileListItems === null) {
      // todo spinner as component
      toRender = ([
        <div className="container-fluid d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <span>Initializing</span>
        </div>
      ]);
    } else {
      toRender = ([
        <div key="FileList" className="row">
          <FileList fileListItems={this.props.fileListItems} />
        </div>,
        <div key="FileListButtons" className="row">
          <FileListButtons getFileListItems={this.props.getFileListItems} loggedInAs={this.props.loggedInAs} />
        </div>
      ]);
    }
    
    return ([
      <div key="Heading" className="row">
        <h1>List of files</h1>
      </div>,
      <div key="Content">
        {toRender}
      </div>
    ]);
  }
}
