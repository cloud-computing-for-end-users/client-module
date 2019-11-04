interface FeatureFlags {
    AllowLogin: boolean;
    ShowConnectionStatus: boolean;
}

let FeatureFlags: FeatureFlags = {
    AllowLogin: false,
    ShowConnectionStatus: true
};

export default FeatureFlags;
