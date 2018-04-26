using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NiceNuget.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace NiceNuget.Api.Infrastructure {
    public class XmlFormatter {

        private readonly string _baseUrl;

        public XmlFormatter(IConfiguration config) {
            _baseUrl = config["BaseUrl"];
        }

        public XElement CreatePackageFeed(IEnumerable<PackageInfo> packages, string version) {
            return new XElement(
                XmlElements.feed,
                new XAttribute(XmlElements.baze, XmlNamespaces.baze),
                new XAttribute(XmlElements.m, XmlNamespaces.m),
                new XAttribute(XmlElements.d, XmlNamespaces.d),
                new XAttribute(XmlElements.georss, XmlNamespaces.georss),
                new XAttribute(XmlElements.gml, XmlNamespaces.gml),
                new XElement(XmlElements.m_count, packages.Count().ToString()),
                packages.Select(package =>
                    new XElement(
                        XmlElements.entry,
                        new XElement(XmlElements.id, $"{_baseUrl}{version}/Packages(Id='{package.Id}',Version='{package.LatestVersion}')"),
                        new XElement(
                            XmlElements.content,
                            new XAttribute("type", "application/zip"),
                            new XAttribute("src", $"{_baseUrl}{version}/download/{package.LatestContentId}")
                        ),
                        new XElement(
                            XmlElements.m_properties,
                            new XElement(XmlElements.d_id, package.Id),
                            new XElement(XmlElements.d_version, package.LatestVersion)
                        )
                    )
                )
            );
        }

        public XElement CreateVersionFeed(string id, VersionInfo[] versions, string apiVersion) {
            return new XElement(
                XmlElements.feed,
                new XAttribute(XmlElements.baze, XmlNamespaces.baze),
                new XAttribute(XmlElements.m, XmlNamespaces.m),
                new XAttribute(XmlElements.d, XmlNamespaces.d),
                new XAttribute(XmlElements.georss, XmlNamespaces.georss),
                new XAttribute(XmlElements.gml, XmlNamespaces.gml),
                new XElement(XmlElements.m_count, versions.Length.ToString()),
                versions.Select(x =>
                    new XElement(
                        XmlElements.entry,
                        new XElement(XmlElements.id, $"{_baseUrl}{apiVersion}/Packages(Id='{id}',Version='{x.Version}')"),
                        new XElement(
                            XmlElements.content,
                            new XAttribute("type", "application/zip"),
                            new XAttribute("src", $"{_baseUrl}{apiVersion}/download/{x.ContentId}")
                        ),
                        new XElement(
                            XmlElements.m_properties,
                            new XElement(XmlElements.d_id, id),
                            new XElement(XmlElements.d_version, x.Version)
                        )
                    )
                )
            );
        }

        public XElement CreatePackageEntry(string packageId, string contentId, Version packageVersion, string apiVersion) {
            return new XElement(XmlElements.entry,
                new XAttribute(XmlElements.baze, XmlNamespaces.baze),
                new XAttribute(XmlElements.m, XmlNamespaces.m),
                new XAttribute(XmlElements.d, XmlNamespaces.d),
                new XAttribute(XmlElements.georss, XmlNamespaces.georss),
                new XAttribute(XmlElements.gml, XmlNamespaces.gml),
                new XElement(XmlElements.id, $"{_baseUrl}{apiVersion}/Packages(Id='{packageId}',Version='{packageVersion}')"),
                new XElement(
                    XmlElements.content,
                    new XAttribute("type", "application/zip"),
                    new XAttribute("src", $"{_baseUrl}{apiVersion}/download/{contentId}")
                ),
                new XElement(
                    XmlElements.m_properties,
                    new XElement(XmlElements.d_id, packageId),
                    new XElement(XmlElements.d_version, packageVersion)
                )
            );
        }
    }
}