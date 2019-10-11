import * as React from "react";
import {EntryForm} from "./EntryForm";
import {EntrySwitchButton} from "./EntrySwitchButton";

interface IState {
  entryType: EntryType
}

interface IProps { 
  onLoggedInChange: any
}

export enum EntryType {
  Login,
  CreateAccount
}

export class Entry extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleEntryFormChange = this.handleEntryFormChange.bind(this);
    this.state = { 
      entryType: EntryType.Login // default Login
    };
  }

  private handleEntryFormChange() {
    if(this.state.entryType === EntryType.Login) {
      this.setState({
        entryType: EntryType.CreateAccount
      })
    } else {
      this.setState({
        entryType: EntryType.Login
      })
    }
  }

  public render(): React.ReactNode {
    return (
      <div className="container login-container">
        <EntryForm onLoggedInChange={this.props.onLoggedInChange} entryType={this.state.entryType} />
        <EntrySwitchButton onEntryFormChange={this.handleEntryFormChange} entryType={this.state.entryType} />
      </div>
    );
  }
}
