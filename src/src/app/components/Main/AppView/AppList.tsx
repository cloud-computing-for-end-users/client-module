import * as React from "react";
import {AppListItem} from "./AppListItem";

interface IState { }

interface IProps { }

export class AppList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    // todo get real values
    var values = ["App 1", "App 2"];
    const items = values.map((value) =>
      <AppListItem key={value} appName={value} />
    );
    
    return (
      <ul id="app-list" className="list-group">
        {items}
      </ul>
    );
  }
}
