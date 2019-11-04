import FeatureFlags from "./FeatureFlags";

export class Utils {
    // Constants
    static readonly defaultPrimaryKey: number = 1;

    static getLoggedInAs(loggedInAs: number): number {
        return FeatureFlags.AllowLogin ? loggedInAs : this.defaultPrimaryKey;
    }

    static guidGenerator() {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + "-" + S4() + "-" + S4() + "-" + S4());
    }
}