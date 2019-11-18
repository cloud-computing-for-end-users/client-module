interface FeatureFlags {
    AllowLogin: boolean;
    ShowConnectionStatus: boolean;
}

let FeatureFlags: FeatureFlags = {
    AllowLogin: true,
    ShowConnectionStatus: true
};

export default FeatureFlags;
