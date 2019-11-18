import * as React from "react";
import * as $ from "jquery";
import {Navigation} from "./Navigation/Navigation";
import {AppView} from "./AppView/AppView";
import {FileView} from "./FileView/FileView";
import {Entry} from "./Entry/Entry";
import {WindowControls} from "../Shared/WindowControls";
import FeatureFlags from "../../../utils/FeatureFlags";
import { ipcRenderer } from "electron";
import {FileListItem} from "./FileView/FileListItem";
import {BackendMethods} from "../../renderer";
import { Utils } from "../../../utils/Utils";

interface IState {
  content: ContentType,
  loggedIn: boolean,
  loggedInAs: number,
  loggedInAsEmail: string,
  fileListItems: any
}

interface IProps { }

export enum ContentType {
  AppView = "Apps",
  FileView = "Files"
}

export class Main extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleViewChange = this.handleViewChange.bind(this);
    this.handleLoggedInChange = this.handleLoggedInChange.bind(this);
    this.getFileListItems = this.getFileListItems.bind(this);
    this.state = { 
      content: ContentType.AppView, // default Apps tab
      loggedIn: false,
      loggedInAs: 0,
      loggedInAsEmail: "",
      fileListItems: null
    };  
  }

  disableDragging(): void {
    $('img').prop('draggable', false);
    $('a').prop('draggable', false);
  }

  componentDidUpdate(): void {
    this.disableDragging();
  }

  componentDidMount(): void {
    this.disableDragging();
    ipcRenderer.on('updateListOfFiles', (event, arg) => {
      this.getFileListItems();
    })
  }

  getFileListItems() {    
    let doLoad: boolean = false;
    if(FeatureFlags.AllowLogin) {
      if(this.state.loggedIn) {
        doLoad = true;
      }
    } else {
      doLoad = true;
    }

    if(doLoad) {
      ipcRenderer.send('call-backend-method', {method: BackendMethods.GetListOfFiles, argument: {PrimaryKey: Utils.getLoggedInAs(this.state.loggedInAs)}});
      ipcRenderer.on('reply-backend-method-' + BackendMethods.GetListOfFiles, (event, arg) => {
        this.setState({
          fileListItems: JSON.parse(arg)
        });
      })
    }
  }

  handleViewChange(content: ContentType): void {
    this.setState({content});
  }  

  handleLoggedInChange(loggedIn: boolean, loggedInAs: number, loggedInAsEmail: string): void {
    this.setState({loggedIn, loggedInAs, loggedInAsEmail, content: ContentType.AppView, fileListItems: null});
  }

  private GetAfterLoginView() : any {
    var view;
    switch(this.state.content) {
      case ContentType.AppView: view = <AppView loggedInAs={this.state.loggedInAs} />; break;
      case ContentType.FileView: view = <FileView loggedInAs={this.state.loggedInAs} fileListItems={this.state.fileListItems} getFileListItems={this.getFileListItems} />; break;
    }
    return [
      <Navigation key="Navigation" email={this.state.loggedInAsEmail} active={this.state.content} onViewChange={this.handleViewChange} onLoggedInChange={this.handleLoggedInChange} />,
      <div key="MainView" className="container mx-3">
        {view}    
      </div>
    ];
  }

  render(): React.ReactNode {
    var toRender;
    if(FeatureFlags.AllowLogin) {
      if(this.state.loggedIn) {
        toRender = this.GetAfterLoginView();
      } else {
        toRender = <Entry key="Entry" onLoggedInChange={this.handleLoggedInChange} />;
      }
    } else {
      toRender = this.GetAfterLoginView();
    }
    
    return ( [ 
        <WindowControls onCloseSlaveAppWindow={null} showDragControl={true} key="WindowControls" />,
        toRender
      ]
    );
  }
}