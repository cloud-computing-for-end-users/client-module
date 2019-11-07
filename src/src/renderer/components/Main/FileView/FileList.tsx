import * as React from "react";
import {FileListItem} from "./FileListItem";

interface IState {}

interface IProps {
  fileListItems: any
}

export class FileList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  render(): React.ReactNode {    
    var items = [];
    var json = this.props.fileListItems;
    for (var i = 0; i < json.length; i++){
      var obj = json[i];
      items.push(<FileListItem key={obj["FileNameProp"]} fileName={obj["FileNameProp"]} />)
    }
    // todo show message when having 0 files
    
    return (
      <ul id="file-list" className="list-group list">
        {items}
      </ul>
    );
  }
}
