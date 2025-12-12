namespace SkyBox.API.Contracts.FileVersions;

public record SaveOrVersionResult(UploadedFile File, bool AlreadyPersisted);
