import * as React from "react";
import * as $ from "jquery";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');

interface IState {}

interface IProps {
  loggedInAs: number
}

export class FileListButtons extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClickDownload = this.handleOnClickDownload.bind(this);
    this.handleOnClickUpload = this.handleOnClickUpload.bind(this);
    this.handleOnClickRename = this.handleOnClickRename.bind(this);
    this.handleOnClickSend = this.handleOnClickSend.bind(this);
  }

  private handleOnClickDownload() {
    alert("Not implemented download");
  }

  private handleOnClickUpload() {
    alert("Not implemented upload");
  }

  private handleOnClickRename() {
    alert("Not implemented rename");
  }

  private handleOnClickSend() {
    let span = $("#file-list li.active div span");
    if(span.length === 1) {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.TellSlaveToFetchFile, 
        argument: {
          SlaveKey: "ALL", // todo specific slave
          FileName: span[0].innerText
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.TellSlaveToFetchFile, (event, arg) => {
        console.log(arg);
        // Sent (TellSlaveToFetchFile)
      })
    }
  }

  public render(): React.ReactNode {    
    // todo change buttons to active / non-active if a file is selected
    
    return (
      <div id="file-list-buttons">
        <button onClick={this.handleOnClickDownload} className="btn btn-outline-primary">Download file</button>
        <button onClick={this.handleOnClickUpload} className="btn btn-outline-primary">Upload new file</button>
        <button onClick={this.handleOnClickRename} className="btn btn-outline-primary">Rename file</button>
        <button onClick={this.handleOnClickSend} className="btn btn-outline-primary">Send file to an application</button>
      </div>
    );
  }
}
