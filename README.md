<h1 align="center">Jellyfin TubeArchivist Metadata Plugin</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org">Jellyfin Community</a></h3>

<p align="center">
Jellyfin TubeArchivist Metadata Plugin helps you to synchronise metadata from TubeArchivist
</p>

`jellyfin-tubearchivist-metadata-plugin` is a Jellyfin plugin designed
to synchronize and manage video metadata from TubeArchivist within
your Jellyfin media server. This plugin provides Jellyfin with the
ability to incorporate TubeArchivist's rich metadata, ensuring your
self-hosted videos are up-to-date and seamlessly integrated with your
Jellyfin library.

## Features

- **Metadata Import**: Automatically import video details from
  TubeArchivist into Jellyfin, enriching your media library with rich
  metadata.
- **Thumbnail and Artwork Sync**: Sync video thumbnails, posters, and
  other artwork from TubeArchivist to Jellyfin.
- **Automated Synchronization**: Set the plugin to periodically
  synchronize metadata without manual intervention.
- **Seamless Integration**: Designed to work within the Jellyfin
  ecosystem, following the familiar settings and configurations
  paradigm.

## Install Process

## From Repository
1. In Jellyfin, go to dashboard -> plugins -> Repositories -> add and
   paste this link
   https://raw.githubusercontent.com/snailwaffles/jellyfin-tubearchivist-metadata-plugin/master/manifest.json
2. Go to Catalog and search for TubeArchivist
3. Click on it and install
4. Restart Jellyfin

## From .zip file
1. Download the .zip file from release page.
2. Extract it and place the .dll file in a folder called
   ```plugins/TubeArchivist``` under the program data directory or
   inside the portable install directory.
3. Restart Jellyfin.

## Configuration

Configure the plugin with the necessary details to connect to your
TubeArchivist instance:

1. Access the `jellyfin-tubearchivist-metadata-plugin` settings
   through the Jellyfin dashboard `Plugins` section.
2. Provide your TubeArchivist instance URL and API key.
3. Save your configuration.

## Usage

With the plugin installed and configured, Jellyfin will now utilize
the metadata from TubeArchivist for the relevant media libraries
during the library scan process. You can manually trigger a library
scan or wait for the next scheduled scan to see the results of the
synchronization.

## License

This `jellyfin-tubearchivist-metadata-plugin` is available under the
[GPLv3 license](LICENSE.md).

## Author

- [snailwaffles](https://github.com/snailwaffles) - Initial work and
  maintenance

## Acknowledgments

- [TubeArchivist](https://github.com/tubearchivist/tubearchivist)
- [Jellyfin Project](https://jellyfin.org/)
- And all our contributors and users who test, use, and support the
  plugin.
