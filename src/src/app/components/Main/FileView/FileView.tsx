import * as React from "react";
import {FileList} from "./FileList";
import {FileListButtons} from "./FileListButtons";

interface IState {
  message: string;
}

interface IProps { 
  loggedInAs: number
}

export class FileView extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    return ([
      <div key="Heading" className="row">
        <h1>List of files</h1>
      </div>,
      <div key="FileList" className="row">
        <FileList loggedInAs={this.props.loggedInAs} />
      </div>,
      <div key="FileListButtons" className="row">
        <FileListButtons loggedInAs={this.props.loggedInAs} />
      </div>
    ]);
  }
}
