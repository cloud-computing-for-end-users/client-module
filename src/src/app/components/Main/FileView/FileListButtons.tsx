import * as React from "react";
import * as $ from "jquery";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');

interface IState {
  renameFileInput: string
}

interface IProps {
  loggedInAs: number
  getFileListItems: any
}

export class FileListButtons extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClickDownload = this.handleOnClickDownload.bind(this);
    this.handleOnClickUpload = this.handleOnClickUpload.bind(this);
    this.handleOnClickRename = this.handleOnClickRename.bind(this);
    this.handleOnClickSend = this.handleOnClickSend.bind(this);
    this.handleRenameFileInputChange = this.handleRenameFileInputChange.bind(this);

    this.state = {
      renameFileInput: ""
    }
  }

  private handleOnClickDownload() {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.DownloadFile, 
        argument: {
          PrimaryKey: "123", // todo this.props.loggedInAs
          FileName: fileName
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.DownloadFile, (event, arg) => {
        console.log(arg + " - remove me (todo)");
        // todo check that arg is "Done (DownloadFile)" and show it in some way
      })
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private handleOnClickUpload() {
    const { dialog } = require('electron').remote
    dialog.showOpenDialog({
      properties: ['openFile']
    }).then(result => {
      if(result.filePaths.length === 0){
        console.log("Empty, do nothing")
      } else if(result.filePaths.length === 1) {
        ipcRenderer.send('call-backend-method', {
          method: BackendMethods.UploadFile, 
          argument: {
            PrimaryKey: "123", // todo this.props.loggedInAs
            FileName:  result.filePaths[0]
          }
        });
        ipcRenderer.on('reply-backend-method-' + BackendMethods.UploadFile, (event, arg) => {
          if(arg === "Done (UploadFile)") {
            this.props.getFileListItems();
            // todo show successful upload in some way, updating list might be enough though
          }
        })
      } else {
        // todo figure what else could possibly happen?
        alert("Something weird happened; file paths length: " + result.filePaths.length)
      }
    }).catch(err => {
      // todo figure out when this happens?
      console.log(err)
    });
  }

  private handleRenameFileInputChange(event: any): void {
    this.setState({
      renameFileInput: event.target.value
    });
  }

  private handleOnClickRename() {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.RenameFile, 
        argument: {
          PrimaryKey: "123", // todo this.props.loggedInAs
          OldFileName: fileName,
          NewFileName: this.state.renameFileInput
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.RenameFile, (event, arg) => {
        console.log(arg + " - remove me (todo)");
        // todo check that arg is "Done (RenameFile)" and show it in some way
        this.props.getFileListItems();
      })
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private handleOnClickSend() {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.TellSlaveToFetchFile, 
        argument: {
          SlaveKey: "ALL", // todo specific slave
          FileName: fileName
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.TellSlaveToFetchFile, (event, arg) => {
        console.log(arg + " - remove me (todo)");
        // todo check that arg is "Sent (TellSlaveToFetchFile)" and show it in some way
      })
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private getActiveFileName() {
    let span = $("#file-list li.active div span");
    if(span.length === 1) { return span[0].innerText }
    else { return "" }
  }

  render(): React.ReactNode {    
    // todo change buttons to active / non-active if a file is selected
    // todo make the button layout prettier
    return (
      <div id="file-list-buttons">
        <button onClick={this.handleOnClickDownload} className="btn btn-outline-primary">Download file</button>
        <button onClick={this.handleOnClickUpload} className="btn btn-outline-primary">Upload new file</button>
        <div className="d-flex flex-column">
          <input value={this.state.renameFileInput} onChange={this.handleRenameFileInputChange} type="text" className="form-control" placeholder="New file name" />
          <button onClick={this.handleOnClickRename} className="btn btn-outline-primary">Rename file</button>
        </div>
        <button className="btn btn-outline-primary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Send file to an application
        </button>
        <div className="dropdown-menu" aria-labelledby="dropdownMenuButton">
          <a className="dropdown-item" onClick={this.handleOnClickSend} href="#">Action</a>
          <a className="dropdown-item" onClick={this.handleOnClickSend} href="#">Another action</a>
          <a className="dropdown-item" onClick={this.handleOnClickSend} href="#">Something else here</a>
        </div>
      </div>
    );
  }
}
