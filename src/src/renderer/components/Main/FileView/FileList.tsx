import * as React from "react";

interface IState {}

interface IProps {
  fileListItems: any
}

export class FileList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  render(): React.ReactNode {    
    return (
      <ul id="file-list" className="list-group list">
        {this.props.fileListItems}
      </ul>
    );
  }
}
