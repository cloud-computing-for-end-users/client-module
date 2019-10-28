import * as React from "react";
import {FileListItem} from "./FileListItem";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');
    
const spinner = require('../../../../../assets/svg/spinner.svg');

interface IState {
  fileListItems: any
}

interface IProps {
  loggedInAs: number
}

var savedFileListItems : any = null;

export class FileList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);

    if(savedFileListItems === null) {
      this.state = {fileListItems: 
        <div className="container-fluid d-flex justify-content-center align-items-center">
          <img draggable={false} src={spinner} />
          <span>Initializing</span>
        </div>
      };
      this.getFileListItems();
    } else {
      this.state = {fileListItems: savedFileListItems};
    }
  }

  private getFileListItems() {    
    ipcRenderer.send('call-backend-method', {method: BackendMethods.GetListOfFiles, argument: {PrimaryKey: "123"}}); // todo this.props.loggedInAs
    ipcRenderer.on('reply-backend-method-' + BackendMethods.GetListOfFiles, (event, arg) => {
      arg = `[
        {
          "FileNameProp": "renamed.txt"
        },
        {
          "FileNameProp": "testUpload2.txt"
        }
      ]`
      var json = JSON.parse(arg);
      var items = [];
      for (var i = 0; i < json.length; i++){
        var obj = json[i];
        items.push(<FileListItem key={obj["FileNameProp"]} fileName={obj["FileNameProp"]} />)
      }
      // todo show message when having 0 files
      savedFileListItems = items;
      this.setState({
        fileListItems: savedFileListItems
      });
    })
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.GetListOfFiles);
  }

  public render(): React.ReactNode {    
    return (
      <ul id="file-list" className="list-group list">
        {this.state.fileListItems}
      </ul>
    );
  }
}
