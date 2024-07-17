# Configuring Instrument Sessions

The Semiconductor Test Library makes an attempt to configure instruments in the most efficient way possible by consolidating the most common drive properties into one a single class. An instance of this class can be created and configured with only the properties intended to be updated, and then operate on that object to update the driver within in one go. This minimizes test time as it minimizes the use of parallel for loops that get called under-the-hood.

## How it works

It always aborts the session. Does not re-initiate or commit the sessions (this is expected to happen in proceeding code).'
