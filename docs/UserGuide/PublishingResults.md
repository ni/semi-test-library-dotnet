# Publishing Results

The Semiconductor Test Library does not directly process or evaluate test results. It instead passes published test results to be separate results handler. The library currently leverages the TestStand Semiconductor Module (TSM) to act its results handler, where the test results are evaluated against limits at the TestStand level. Refer to the TSM documentation linked below for more information.

> [!NOTE]
> The Semiconductor Test Library provides extension methods to enable easier publishing of results and add support for the SiteData and PinSiteData types. These methods extend TSM’s ISemiconductorModule context object. Refer to the `Publish` class in API Reference for more details.
>
> Namespace: `NationalInstruments.SemiconductorTestLibrary.Common`

**Related information:**

- [NI TSM: Publishing Results](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/publishing-results.html)
