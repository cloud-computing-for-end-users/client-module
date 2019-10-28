import * as React from "react";
import * as $ from "jquery";
import {Navigation} from "./Navigation/Navigation";
import {AppView} from "./AppView/AppView";
import {FileView} from "./FileView/FileView";
import {Entry} from "./Entry/Entry";
import {WindowControls} from "../Shared/WindowControls";
import FeatureFlags from "../../FeatureFlags";


interface IState {
  content: ContentType,
  loggedIn: boolean,
  loggedInAs: number
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
    this.state = { 
      content: ContentType.AppView, // default Apps tab
      loggedIn: false,
      loggedInAs: 0
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
  }

  handleViewChange(content: ContentType): void {
    this.setState({content});
  }  

  handleLoggedInChange(loggedIn: boolean, loggedInAs: number): void {
    this.setState({loggedIn, loggedInAs});
  }

  private GetAfterLoginView() : any {
    var view;
    switch(this.state.content) {
      case ContentType.AppView: view = <AppView loggedInAs={this.state.loggedInAs} />; break;
      case ContentType.FileView: view = <FileView loggedInAs={this.state.loggedInAs} />; break;
    }
    return [
      <Navigation key="Navigation" active={this.state.content} onViewChange={this.handleViewChange} onLoggedInChange={this.handleLoggedInChange} />,
      <div key="MainView" className="container mx-3">
        {view}    
      </div>
    ];
  }

  public render(): React.ReactNode {
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
        <WindowControls slaveAppKey={null} showDragControl={true} key="WindowControls" />,
        toRender
      ]
    );
  }
}