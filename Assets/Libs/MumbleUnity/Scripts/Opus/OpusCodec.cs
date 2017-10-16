﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mumble 
{
    public class OpusCodec
    {
        readonly OpusDecoder _decoder = new OpusDecoder((int)MumbleConstants.SAMPLE_RATE, MumbleConstants.NUM_CHANNELS) { EnableForwardErrorCorrection = false };
        private OpusEncoder _encoder;


        public IEnumerable<int> PermittedEncodingFrameSizes
        {
            get
            {
                return _encoder.PermittedFrameSizes;
            }
        }

        public ArraySegment<byte> Encode(float[] pcmData)
        {
            return _encoder.Encode(pcmData);
        }
        public int Decode(byte[] encodedData, float[] floatBuffer)
        {
            return _decoder.Decode(encodedData, floatBuffer);
        }
        public bool InitializeEncoderWithSampleRate(int newSampleRate)
        {
            if (_encoder != null)
                return false;

            _encoder = new OpusEncoder(newSampleRate, 1) { EnableForwardErrorCorrection = false };
            return _encoder != null;
        }
    }
}
