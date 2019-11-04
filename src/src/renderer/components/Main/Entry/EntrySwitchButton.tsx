import * as React from "react";
import {EntryType} from "./Entry";

interface IState { }

interface IProps {
    entryType: EntryType,
    onEntryFormChange: any
}

export class EntrySwitchButton extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClick = this.handleOnClick.bind(this); 
  }

  private handleOnClick() : void {
    this.props.onEntryFormChange();
  }

  render(): React.ReactNode {
    var buttonText, helpText;
    switch(this.props.entryType) {
      case EntryType.Login: 
        buttonText = "Create account";
        helpText = "Don't have an account?";
        break;
      case EntryType.CreateAccount:
        buttonText = "Login";
        helpText = "Already have an account?";
        break;
    }
    
    return (
      <div className="d-flex flex-column justify-content-center">
        <span className="mt-3 mb-2">{helpText}</span>
        <button onClick={this.handleOnClick} className="btn btn-outline-secondary">{buttonText}</button>
      </div>
    );
  }
}
