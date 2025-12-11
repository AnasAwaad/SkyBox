namespace SkyBox.API.Contracts.FileVersion;

public record SaveOrVersionResult(UploadedFile File, bool AlreadyPersisted);
