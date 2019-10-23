import * as React from "react";
import {AppList} from "./AppList";

interface IState { }

interface IProps {
  primaryKey: number
}

export class AppView extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    return ([
      <div key="Heading" className="row mx-2">
        <h1>List of apps</h1>
      </div>,
      <div key="AppList" className="row mx-2">
        <AppList primaryKey={this.props.primaryKey} />
      </div>
    ]);
  }
}
